set SCRIPT_HOME=%~dp0.

pushd %SCRIPT_HOME%

set ycmd="gp\Gppg.exe"
set lcmd="gp\Gplex.exe"

REM %ycmd% /noinfo /no-lines /gsw-nullablereferencetypes "Dot.y"  > "DotParser.cs"
%lcmd% /noinfo /gsw-nullablereferencetypes /frame:"Serialization\DotLexFrame.cs" /out:"Serialization\DotLex.cs" "Serialization\Dot.lex"

popd