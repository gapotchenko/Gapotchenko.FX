set SCRIPT_HOME=%~dp0.

pushd %SCRIPT_HOME%

set ycmd="gp\Gppg.exe"
set lcmd="gp\Gplex.exe"

%ycmd% /noinfo /no-lines /gsw-nullablereferencetypes "Dot.y"  > "DotParser.cs"
%lcmd% /noinfo /gsw-nullablereferencetypes /frame:"DotLexFrame.cs" /out:"DotLex.cs" "Dot.lex"

popd