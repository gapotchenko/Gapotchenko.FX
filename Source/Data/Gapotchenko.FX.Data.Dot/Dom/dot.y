%using Gapotchenko.FX.Data.Dot.ParserToolkit;
%namespace Gapotchenko.FX.Data.Dot.Dom
%visibility internal
%parsertype DotParser
%tokentype DotTokens
%YYSTYPE DotValueType
%partial

%start graph

%union {
    public DotParser.DotToken token;
    public DotNode entity;
    public SeparatedDotNodeList<DotNode, DotArrowToken> separatedSyntaxList;
    public IList<DotAttributeNode> attributeSyntaxList;
    public IList<DotAttributeListNode> attributeListSyntaxList;
    public IList<DotStatementNode> statementSyntaxList;
}

%token <token> DIGRAPH GRAPH ARROW SUBGRAPH NODE EDGE ID

%type <token> graphType graphName id
%type <entity> graph subgraph stmt stmt node_stmt edge_stmt endpoint stmts node_id
%type <entity> attr_stmt opt_attr_list avPair
%type <statementSyntaxList> stmt_list
%type <separatedSyntaxList> edgeLHS
%type <attributeSyntaxList> a_list
%type <attributeListSyntaxList> attr_list opt_attr_list

%left '{'

%%

graph     : graphType graphName stmts    { Root = CreateGraphSyntax(default, $1, $2, (DotStatementListNode)$3); }
          | id graphType graphName stmts { Root = CreateGraphSyntax($1, $2, $3, (DotStatementListNode)$4); }
          ;
          
stmts     : '{' stmt_list '}' { $$ = CreateStatementListSyntax($1.token, $2, $3.token); }
          ;

graphType : DIGRAPH { $$ = $1; }
          | GRAPH   { $$ = $1; }
          ;
 
graphName : id { $$ = $1; } 
          |    { }
          ;

stmt_list : { $$ = new List<DotStatementNode>(); }
          | stmt_list stmt     { $1.Add((DotStatementNode)$2); $$ = $1; }
          | stmt_list stmt ';' { var statement = (DotStatementNode)$2;
                                 statement.SemicolonToken = CreatePunctuationToken($3.token);
                                 $1.Add((DotStatementNode)$2);
                                 $$ = $1; }
          ;

stmt      : id '=' id { $$ = CreateAliasSyntax($1, $2.token, $3); } 
          | node_stmt { $$ = $1; }
          | edge_stmt { $$ = $1; }
          | attr_stmt { $$ = $1; }
          | subgraph  { $$ = $1; }
          ;

node_stmt : node_id opt_attr_list { $$ = CreateVertexSyntax((DotVertexIdentifierNode)$1, $2); }
          ;

edge_stmt : edgeLHS endpoint opt_attr_list { $1.Add($2); $$ = CreateEdgeSyntax($1, $3);} 
          ;

endpoint  : node_id  { $$ = $1; }
          | subgraph { $$ = $1; }
		  ;

edgeLHS   : endpoint ARROW          { $$ = new(CreateArrowToken($2)) { $1 }; }
          | edgeLHS endpoint ARROW  { $1.Add($2, CreateArrowToken($3)); $$ = $1; }
          ;

subgraph  : stmts              { $$ = CreateGraphSyntax(default, default, default, (DotStatementListNode)$1); }
          | SUBGRAPH stmts     { $$ = CreateGraphSyntax(default, $1, default, (DotStatementListNode)$2); }
          | SUBGRAPH id stmts  { $$ = CreateGraphSyntax(default, $1, $2, (DotStatementListNode)$3); }
          | SUBGRAPH           { $$ = CreateGraphSyntax(default, $1, default, default); }
          ;

attr_stmt : GRAPH attr_list { $$ = CreateAttachedAttributesSyntax($1, $2); }
          | NODE attr_list  { $$ = CreateAttachedAttributesSyntax($1, $2); }
          | EDGE attr_list  { $$ = CreateAttachedAttributesSyntax($1, $2); }
          ;

opt_attr_list :       { }
          | attr_list { $$ = $1; }
          ;

attr_list : '[' ']'        { $$ = CreateAttributeListSyntaxList($1.token, default, $2.token); }
          | '[' a_list ']' { $$ = CreateAttributeListSyntaxList($1.token, $2, $3.token); }
          ;

a_list    : avPair            { $$ = new List<DotAttributeNode>(); $$.Add((DotAttributeNode)$1); }
          | a_list avPair     { $$ = $1; $1.Add((DotAttributeNode)$2); }
          | a_list avPair ',' { $$ = $1;
                                var attr = (DotAttributeNode)$2;
                                attr.SemicolonOrCommaToken = CreatePunctuationToken($3.token); 
                                $1.Add(attr); }
          | a_list avPair ';' { $$ = $1; 
                                var attr = (DotAttributeNode)$2;
                                attr.SemicolonOrCommaToken = CreatePunctuationToken($3.token); 
                                $1.Add(attr); }
          ;

avPair    : id '=' id { $$ = CreateAttributeSyntax($1, $2.token, $3, default); }
          | id        { $$ = CreateAttributeSyntax($1, default, default, default); }
          ;

node_id   : id               { $$ = CreateVertexIdentifierSyntax($1, default, default, default, default); }
          | id ':' id        { $$ = CreateVertexIdentifierSyntax($1, $2.token, $3, default, default); }
          | id ':' id ':' id { $$ = CreateVertexIdentifierSyntax($1, $2.token, $3, $4.token, $5); }
          ;
          
id        : ID          { $$ = $1; }
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
