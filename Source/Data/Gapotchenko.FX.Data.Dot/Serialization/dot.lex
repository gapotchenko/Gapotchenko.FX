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
\<                 { BEGIN(HTML); nesting = 1; BuilderInit(); BuilderAppend(); }
\#                 { BEGIN(LINECOMMENT); BuilderInit(); BuilderAppend(); }
{whitespace}+      { return (int)DotTokenKind.Whitespace; }
\/\/               { BEGIN(LINECOMMENT); BuilderInit(); BuilderAppend(); } 
\/\*               { BEGIN(MLINECOMMENT); BuilderInit(); BuilderAppend(); }
.                  { Error(yytext); }

<HTML>\<           { nesting++; 
                     BuilderAppend(); }
<HTML>\>           { nesting--; 
                     BuilderAppend(); 
                     if (nesting == 0) {
                        BEGIN(INITIAL);
                        tokTxt = BuilderBuild();
                        return (int)DotTokenKind.Id;
                     } 
                   }
<HTML>[^<>]        { BuilderAppend(); }

<LINECOMMENT>(\r|\n)    { _yytrunc(1); tokTxt = BuilderBuild(); BEGIN(INITIAL); return (int)DotTokenKind.Comment; }
<LINECOMMENT>[^\r\n]    { BuilderAppend(); } 

<MLINECOMMENT>\*\/      { BuilderAppend(); tokTxt = BuilderBuild(); BEGIN(INITIAL); return (int)DotTokenKind.MultilineComment; }
<MLINECOMMENT>(.|\n)    { BuilderAppend(); }

<STRING>([^"]*)(\\\")   { stringId += yytext; }
<STRING>([^"]*)+        { stringId += yytext;} 
<STRING>\"              { _yytrunc(1); BEGIN(STRINGQ); tokTxt = stringId; return (int)DotTokenKind.Id; }
<STRINGQ>\"             { BEGIN(INITIAL); return (int)DotTokenKind.Quote; }


%{
    /* Epilog from LEX file */
    LoadYylVal();
%}

// =============================================================
%% // Start of user code
// =============================================================
