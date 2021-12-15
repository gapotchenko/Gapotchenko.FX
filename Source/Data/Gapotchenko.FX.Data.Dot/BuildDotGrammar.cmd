set SCRIPT_HOME=%~dp0.

pushd %SCRIPT_HOME%

set ycmd="gp\Gppg.exe"
set lcmd="gp\Gplex.exe"

%ycmd% /noinfo /no-lines /gsw-nullablereferencetypes "Dom\Dot.y"  > "Dom\DotParser.cs"
%lcmd% /noinfo /unicode /gsw-nullablereferencetypes /frame:"Serialization\DotLexFrame.cs" /out:"Serialization\DotLex.cs" "Serialization\Dot.lex"

popd