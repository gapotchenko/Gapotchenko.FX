%using Gapotchenko.FX.Math.Topology.Serialization.ParserToolkit;
%namespace Gapotchenko.FX.Math.Topology.Serialization
%visibility internal
%parsertype DotParser
%tokentype DotTokens
%YYSTYPE DotValueType
%partial

%start graph

%union {
    public string? sVal;
    public KeyValuePair<string, string> avPair;
    public DotParser.Cell<DotDocumentVertex> sList;
	public DotParser.Cell<DotParser.Cell<DotDocumentVertex>> sLists;
    public List<KeyValuePair<string, string>>? attributes;
    public DotDocumentVertex vertex;
}

%token DIGRAPH GRAPH ARROW SUBGRAPH NODE EDGE
%token <sVal> ID

%left '{'

%%

graph     :  graphType graphName '{' stmt_list '}'
          ;



graphType : DIGRAPH { _strict = false; _directed = true; }
          | GRAPH   { _strict = false; _directed = false; }
		  | ID DIGRAPH { _strict = $1 != null && "strict" == $1!.ToLower(); _directed = true; }
          | ID GRAPH   { _strict = $1 != null && "strict" == $1!.ToLower(); _directed = false; }          
          ;
 
graphName : ID { _graphId = $1; } 
          |    { }
          ;

stmt_list : { $$.sList = new(); }
          | stmt stmt_list { $$.sList = Append($1.sList, $2.sList); }
          | stmt ';' stmt_list { $$.sList = Append($1.sList, $3.sList); }
          ;

stmt      : ID '=' ID { MkEqStmt($1, $3); $$.sList = new(); } 
          | node_stmt { $$.sList = $1.sList; }
          | edge_stmt { $$.sList = $1.sList; }
          | attr_stmt { $$.sList = new(); }
          | subgraph  { $$.sList = $1.sList; }
          ;

node_stmt : node_id opt_attr_list { $$.sList = MkVertexStmt($1.vertex, $2.attributes); }
          ;

edge_stmt : endpoint edgeRHS opt_attr_list { $$.sList = MkEdgeStmt($1.sList, $2.sLists, $3.attributes);} 
          ;

endpoint  : node_id  { $$.sList = MkSingleton($1.vertex); }
          | subgraph { $$.sList = $1.sList; }
		  ;

edgeRHS   : ARROW endpoint  { $$.sLists = MkSingleton($2.sList); }
          | edgeRHS ARROW endpoint  { $$ = $1; $$.sLists = new($$.sLists, $3.sList, $3.sLists); }
          ;

subgraph  : '{'  stmt_list '}' { $$.sList = $2.sList; }
          | SUBGRAPH  '{' stmt_list '}' { $$.sList = $3.sList;}
          | SUBGRAPH  id  {CreateNewCurrentSubgraph($2.sVal); } '{' stmt_list '}' { $$.sList = new(); 
		                        PopCurrentSubgraph(); }
          | SUBGRAPH  { $$.sList = new(); }
          ;

attr_stmt : GRAPH attr_list { MkGraphAttrStmt($2.attributes); }
          | NODE attr_list { MkVertexAttrStmt($2.attributes); }
          | EDGE attr_list { MkEdgeAttrStmt($2.attributes); }
          ;

opt_attr_list : { $$.attributes = new(); }
          | attr_list { $$ = $1; }
          ;

attr_list : '[' ']' { $$.attributes = new(); }
          | '[' a_list ']' { $$ = $2; }
          ;

a_list    : avPair            { $$.attributes = new(); $$.attributes.Add($1.avPair); }
          | avPair a_list     { $2.attributes!.Add($1.avPair); $$ = $2; }
          | avPair ',' a_list { $3.attributes!.Add($1.avPair); $$ = $3; }
          ;

avPair    : id '=' id { $$.avPair = MkAvPair($1.sVal!, $3.sVal!); }
          | id { $$.avPair = MkAvPair($1.sVal!, ""); }
          ;

node_id   : id opt_port { $$.vertex = MkVertex($1.sVal!); /* ignore port */ }
          ;

opt_port  : {}
          | port {}
          ;

port      : port_location {}
          | port_angle {}
          | port_angle port_location {}
          | port_location port_angle {}
          ;

port_location : ':' id {}
          | ':' '(' id ',' id ')' {}
          ;

id        : ID { $$.sVal = $1; }
          ;

port_angle : '@' id { $$.sVal = $2.sVal; }
          ;

%%

/*

port : ':' ID [ ':' compass_pt ] 
     | ':' compass_pt 
subgraph : [ subgraph [ ID ] ] '{' stmt_list '}' 
compass_pt : (n | ne | e | se | s | sw | w | nw | c | _) 

The keywords node, edge, graph, digraph, subgraph, and strict are case-independent. 
Note also that the allowed compass point values are not keywords,
so these strings can be used elsewhere as ordinary identifiers and, conversely, 
the parser will actually accept any identifier. 

An ID is one of the following: 

Any string of alphabetic ([a-zA-Z\200-\377]) characters, underscores ('_') or digits ([0-9]), not beginning with a digit; 
a numeral [-]?(.[0-9]+ | [0-9]+(.[0-9]*)? ); 
any double-quoted string ("...") possibly containing escaped quotes (\")1; 
an HTML string (<...>). 
An ID is just a string; the lack of quote characters in the first two forms is just for simplicity. There is no semantic difference between abc_2 and "abc_2", or between 2.34 and "2.34". Obviously, to use a keyword as an ID, it must be quoted. Note that, in HTML strings, angle brackets must occur in matched pairs, and unescaped newlines are allowed. In addition, the content must be legal XML, so that the special XML escape sequences for ", &, <, and > may be necessary in order to embed these characters in attribute values or raw text. 
Both quoted strings and HTML strings are scanned as a unit, so any embedded comments will be treated as part of the strings. 

An edgeop is -> in directed graphs and -- in undirected graphs. 

An a_list clause of the form ID is equivalent to ID=true. 

The language supports C++-style comments: and //. In addition, a line beginning with a '#' character is considered a line output from a C preprocessor (e.g., # 34 to indicate line 34 ) and discarded. 

Semicolons aid readability but are not required except in the rare case that a named subgraph with no body immediately preceeds an anonymous subgraph, since the precedence rules cause this sequence to be parsed as a subgraph with a heading and a body. Also, any amount of whitespace may be inserted between terminals. 

As another aid for readability, dot allows single logical lines to span multiple physical lines using the standard C convention of a backslash immediately preceding a newline character. In addition, double-quoted strings can be concatenated using a '+' operator. As HTML strings can contain newline characters, they do not support the concatenation operator. 

*/
