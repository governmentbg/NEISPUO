#!/bin/bash

cd /app/Neispuo.Tools.Report/ && dotnet Neispuo.Tools.Report.dll stats > /proc/1/fd/1 2>/proc/1/fd/2