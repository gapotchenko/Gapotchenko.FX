%using Gapotchenko.FX.Data.Dot.ParserToolkit;
%namespace Gapotchenko.FX.Data.Dot.Serialization
%visibility internal
%scannertype DotLex
%tokentype Tokens

%{
    int nesting = 0;
  
    internal void LoadYylVal() {
        yylval = yytext; // tokTxt
    }

%}

alpha [a-zA-Z]
alphaplus [a-zA-Z0-9\-\200-\377\_!\.\'\?]
alphaplain [a-zA-Z0-9\200-\377\_*!\.\'\?]
num [\-]?(\.([0-9]+)|([0-9]+)(\.([0-9]*))?)
whitespace [ \t\r\f\v\n]

%x QSTRING
%x QSTRING_END
%x HTML
%x HTML_END
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
\"                 { BEGIN(QSTRING); BuilderInit(); return (int)DotTokenKind.Quote; }
\[                 { return (int)DotTokenKind.ListStart; }
\]                 { return (int)DotTokenKind.ListEnd; }
\{                 { return (int)DotTokenKind.ScopeStart; }
\}                 { return (int)DotTokenKind.ScopeEnd; }
\,                 { return (int)DotTokenKind.Comma; }
\;                 { return (int)DotTokenKind.Semicolon; }
\:                 { return (int)DotTokenKind.Colon; }
\=                 { return (int)DotTokenKind.Equal; }
\<                 { BEGIN(HTML); nesting = 1; BuilderInit(); return (int)DotTokenKind.HtmlStringStart; }
\#                 { BEGIN(LINECOMMENT); BuilderInit(); BuilderAppend(); }
{whitespace}+      { return (int)DotTokenKind.Whitespace; }
\/\/               { BEGIN(LINECOMMENT); BuilderInit(); BuilderAppend(); } 
\/\*               { BEGIN(MLINECOMMENT); BuilderInit(); BuilderAppend(); }
.                  { Error(yytext); }

<HTML>\<           { nesting++; 
                     BuilderAppend(); }
<HTML>\>           { nesting--; 
                     if (nesting == 0) {
                       _yytrunc(1);
                       BEGIN(HTML_END);
                       tokTxt = BuilderBuild();
                       return (int)DotTokenKind.Id;
                     } else {
                       BuilderAppend(); 
                     }
                   }
<HTML>[^<>]        { BuilderAppend(); }
<HTML_END>\>       { BEGIN(INITIAL); return (int)DotTokenKind.HtmlStringEnd; }

<LINECOMMENT>(\r|\n)    { _yytrunc(1); tokTxt = BuilderBuild(); BEGIN(INITIAL); return (int)DotTokenKind.Comment; }
<LINECOMMENT>[^\r\n]    { BuilderAppend(); } 

<MLINECOMMENT>\*\/      { BuilderAppend(); tokTxt = BuilderBuild(); BEGIN(INITIAL); return (int)DotTokenKind.MultilineComment; }
<MLINECOMMENT>(.|\n)    { BuilderAppend(); }

<QSTRING>\\\\            { BuilderAppend(); }
<QSTRING>\\\"            { BuilderAppend(); }
<QSTRING>([^"\\]+|[\\])  { BuilderAppend(); }
<QSTRING>\"              { _yytrunc(1); BEGIN(QSTRING_END); tokTxt = BuilderBuild(); return (int)DotTokenKind.Id; }
<QSTRING_END>\"          { BEGIN(INITIAL); return (int)DotTokenKind.Quote; }


%{
    /* Epilog from LEX file */
    LoadYylVal();
%}

// =============================================================
%% // Start of user code
// =============================================================
