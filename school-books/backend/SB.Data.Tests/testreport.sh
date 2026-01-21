#!/bin/bash

rm -r TestResults/

dotnet test --collect:"XPlat Code Coverage" --settings:./runsettings.xml

cd TestResults

npx xslt3 -xsl:../xunit.html.xslt -s:TestResults.xml -o:TestResults.html -t
open TestResults.html
