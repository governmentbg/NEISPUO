#!/bin/bash

rm -r TestResults/

dotnet test --collect:"XPlat Code Coverage" --settings:./runsettings.xml

cd TestResults

npx xslt3 -xsl:../xunit.html.xslt -s:TestResults.xml -o:TestResults.html -t
open TestResults.html

# cd into the first dir
cd $(ls -d */|head -n 1)
reportgenerator "-reports:coverage.cobertura.xml" "-targetdir:coveragereport" "-reporttypes:Html;JsonSummary"
open coveragereport/index.html
