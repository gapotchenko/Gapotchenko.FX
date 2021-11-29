%using Gapotchenko.FX.Math.Topology.Serialization.ParserToolkit;
%namespace Gapotchenko.FX.Math.Topology.Serialization
%visibility internal
%scannertype DotScanner
%tokentype DotTokens

%{
    int nesting = 0;
	string? stringId = null;
  
    internal void LoadYylVal() {
        int dummy = yytext.Length;
        yylval.sVal = tokTxt;
    }

%}

alpha [a-zA-Z]
alphaplus [a-zA-Z0-9\-\200-\377\_!\.\'\?]
alphaplain [a-zA-Z0-9\200-\377\_*!\.\'\?]
num [\-]?(\.([0-9]+)|([0-9]+)(\.([0-9]*))?)

%x STRING
%x HTML
%x LINECOMMENT
%x MLINECOMMENT

%%

\-\>               { return (int) DotTokens.ARROW; }
{alphaplain}{alphaplain}* { return (int) MkId(yytext); }
{alphaplain}{alphaplus}*{alphaplain} { return (int) MkId(yytext); }
{num}              { return (int) DotTokens.ID; }
\-\-               { return (int) DotTokens.ARROW; }
\"                 { BEGIN(STRING); stringId = ""; }
\[                 { return (int)'['; }
\]                 { return (int)']'; }
\{                 { return (int)'{'; }
\}                 { return (int)'}'; }
\(                 { return (int)'('; }
\)                 { return (int)')'; }
\,                 { return (int)','; }
\;                 { return (int)';'; }
\:                 { return (int)':'; }
\=                 { return (int)'='; }
\@                 { return (int)'@'; }
\<                 { BEGIN(HTML); nesting = 1; }
\n\#               { BEGIN(LINECOMMENT); }
[ \t\r\f\v\n]	   {}
\/\/               { BEGIN(LINECOMMENT); } 
\/\*               { BEGIN(MLINECOMMENT); }
.                  { Error(yytext); }

<HTML>\<           { nesting++; }
<HTML>\>           { nesting--; if (nesting == 0) { BEGIN(INITIAL); return (int)DotTokens.ID; } }
<HTML>.            { }

<LINECOMMENT>\n    { BEGIN(INITIAL); }
<LINECOMMENT>.     {} 

<MLINECOMMENT>\*\/ { BEGIN(INITIAL); }
<MLINECOMMENT>.    {}

<STRING>([^\n"]*)(\\\r?\n)	{ stringId += yytext; stringId = TrimString(stringId); }
<STRING>([^\n"]*)(\\\")     { stringId += yytext; }
<STRING>([^\n"]*)+          { stringId += yytext;} 
<STRING>\"                  {BEGIN(INITIAL); tokTxt = stringId; return (int)DotTokens.ID; }


%{
    LoadYylVal();
%}

%%

 // TBD: 
 // parse string concatenation

    string message = "";

    public int Line { get { return yyline; }}
    public int Col  { get { return yycol; }}
    public string Message { get { return message; }}

    public override void yyerror(string format, params object[] args)
    {	  
        if (args.Length > 0) 
 	    message = String.Format(format, args);
        else
	    message = format;
    }

    public void Error(string txt) {
       if (txt != null) {
           message = String.Format("Unexpected {0}",txt);
       }
    }

    public static DotTokens MkId(string txt) {
       switch (txt.ToLower()) {
       case "graph":    return DotTokens.GRAPH;
       case "digraph":  return DotTokens.DIGRAPH;
       case "subgraph": return DotTokens.SUBGRAPH;
       case "node":     return DotTokens.NODE;
       case "edge":     return DotTokens.EDGE;
       default:         return DotTokens.ID;
       }       
    }

	static string TrimString(string stringId) {
        if (stringId.EndsWith("\\\r\n"))
            stringId = stringId.Substring(0, stringId.Length - 3);
        else if (stringId.EndsWith("\\\n"))
            stringId = stringId.Substring(0, stringId.Length - 2);
        return stringId;
	}



