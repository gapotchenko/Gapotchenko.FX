%using Gapotchenko.FX.Data.Dot.ParserToolkit;
%namespace Gapotchenko.FX.Data.Dot.Serialization
%visibility internal
%scannertype DotLex
%tokentype DotToken

%{
    int nesting = 0;
    string? stringId = null;
  
    internal void LoadYylVal() {
        yylval = yytext; // tokTxt
    }

%}

alpha [a-zA-Z]
alphaplus [a-zA-Z0-9\-\200-\377\_!\.\'\?]
alphaplain [a-zA-Z0-9\200-\377\_*!\.\'\?]
num [\-]?(\.([0-9]+)|([0-9]+)(\.([0-9]*))?)
whitespace [ \t\r\f\v\n]

%x STRING
%x STRINGQ
%x HTML
%x LINECOMMENT
%x MLINECOMMENT

// =============================================================
%%  // Start of rules
// =============================================================

\-\>               { return (int) DotToken.ARROW; }
{alphaplain}{alphaplain}* { return (int) MkId(yytext); }
{alphaplain}{alphaplus}*{alphaplain} { return (int) MkId(yytext); }
{num}              { return (int) DotToken.ID; }
\-\-               { return (int) DotToken.ARROW; }
\"                 { BEGIN(STRING); stringId = ""; return '"'; }
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
{whitespace}+      { return (int)DotToken.WHITESPACE; }
\/\/               { BEGIN(LINECOMMENT); } 
\/\*               { BEGIN(MLINECOMMENT); }
.                  { Error(yytext); }

<HTML>\<           { nesting++; }
<HTML>\>           { nesting--; if (nesting == 0) { BEGIN(INITIAL); return (int)DotToken.ID; } }
<HTML>.            { }

<LINECOMMENT>\n    { BEGIN(INITIAL); return (int)DotToken.LINECOMMENT; }
<LINECOMMENT>.     {} 

<MLINECOMMENT>\*\/ { BEGIN(INITIAL); return (int)DotToken.MLINECOMMENT; }
<MLINECOMMENT>.    {}

<STRING>([^\n"]*)(\\\r?\n)    { stringId += yytext; stringId = TrimString(stringId); }
<STRING>([^\n"]*)(\\\")     { stringId += yytext; }
<STRING>([^\n"]*)+          { stringId += yytext;} 
<STRING>\"                  { _yytrunc(1); BEGIN(STRINGQ); tokTxt = stringId; return (int)DotToken.ID; }
<STRINGQ>\"                 { BEGIN(INITIAL); return (int)'"'; }


%{
    /* Epilog from LEX file */
    LoadYylVal();
%}

// =============================================================
%% // Start of user code
// =============================================================
