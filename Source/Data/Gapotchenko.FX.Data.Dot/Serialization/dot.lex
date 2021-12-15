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

NonASCII [^\0-\177]
Letter   [A-Za-z_]{+}[[:NonASCII:]]
Digit	 [0-9]
Name	 {Letter}({Letter}|{Digit})*
Number	 [\-]?(({Digit}+(\.{Digit}*)?)|(\.{Digit}+))(\.|{Letter})?
Id		 ({Name}|{Number})
Whitespace [ \t\r\f\v\n]

%x QSTRING
%x HTML
%x LINECOMMENT
%x MLINECOMMENT

// =============================================================
%%  // Start of rules
// =============================================================

\-\>               { return (int) DotTokenKind.Arrow; }
{Id}               { return (int) MkId(yytext); }
\-\-               { return (int) DotTokenKind.Arrow; }
\"                 { BEGIN(QSTRING); BuilderInit(); BuilderAppend(); }
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
{Whitespace}+      { return (int)DotTokenKind.Whitespace; }
\/\/               { BEGIN(LINECOMMENT); BuilderInit(); BuilderAppend(); } 
\/\*               { BEGIN(MLINECOMMENT); BuilderInit(); BuilderAppend(); }
.                  { RaiseUnexpectedCharError(yytext); }

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

<QSTRING>\\\\            { BuilderAppend(); }
<QSTRING>\\\"            { BuilderAppend(); }
<QSTRING>([^"\\]+|[\\])  { BuilderAppend(); }
<QSTRING>\"              { BEGIN(INITIAL); BuilderAppend(); tokTxt = BuilderBuild(); return (int)DotTokenKind.Id; }


%{
    /* Epilog from LEX file */
    LoadYylVal();
%}

// =============================================================
%% // Start of user code
// =============================================================
