%using Gapotchenko.FX.Data.Dot.ParserToolkit;
%namespace Gapotchenko.FX.Data.Dot.Serialization
%visibility internal
%scannertype DotLex
%tokentype Tokens

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

\-\>               { return (int) DotTokenKind.Arrow; }
{alphaplain}{alphaplain}* { return (int) MkId(yytext); }
{alphaplain}{alphaplus}*{alphaplain} { return (int) MkId(yytext); }
{num}              { return (int) DotTokenKind.Id; }
\-\-               { return (int) DotTokenKind.Arrow; }
\"                 { BEGIN(STRING); stringId = ""; return (int)DotTokenKind.Quote; }
\[                 { return (int)DotTokenKind.ListStart; }
\]                 { return (int)DotTokenKind.ListEnd; }
\{                 { return (int)DotTokenKind.ScopeStart; }
\}                 { return (int)DotTokenKind.ScopeEnd; }
\,                 { return (int)DotTokenKind.Comma; }
\;                 { return (int)DotTokenKind.Semicolon; }
\:                 { return (int)DotTokenKind.Colon; }
\=                 { return (int)DotTokenKind.Equal; }
\<                 { BEGIN(HTML); nesting = 1; }
\n\#               { BEGIN(LINECOMMENT); }
{whitespace}+      { return (int)DotTokenKind.Whitespace; }
\/\/               { BEGIN(LINECOMMENT); } 
\/\*               { BEGIN(MLINECOMMENT); }
.                  { Error(yytext); }

<HTML>\<           { nesting++; }
<HTML>\>           { nesting--; if (nesting == 0) { BEGIN(INITIAL); return (int)DotTokenKind.Id; } }
<HTML>.            { }

<LINECOMMENT>\n    { BEGIN(INITIAL); return (int)DotTokenKind.Comment; }
<LINECOMMENT>.     {} 

<MLINECOMMENT>\*\/ { BEGIN(INITIAL); return (int)DotTokenKind.MultilineComment; }
<MLINECOMMENT>.    {}

<STRING>([^\n"]*)(\\\r?\n)    { stringId += yytext; stringId = TrimString(stringId); }
<STRING>([^\n"]*)(\\\")     { stringId += yytext; }
<STRING>([^\n"]*)+          { stringId += yytext;} 
<STRING>\"                  { _yytrunc(1); BEGIN(STRINGQ); tokTxt = stringId; return (int)DotTokenKind.Id; }
<STRINGQ>\"                 { BEGIN(INITIAL); return (int)DotTokenKind.Quote; }


%{
    /* Epilog from LEX file */
    LoadYylVal();
%}

// =============================================================
%% // Start of user code
// =============================================================
