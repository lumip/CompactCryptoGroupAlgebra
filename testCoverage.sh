#!/bin/bash
set -e
dotnet restore
nuget install ReportGenerator -Version 4.6.1 -OutputDirectory testPackages
rm -rf CompactCryptoGroupAlgebra.Tests/Coverage
dotnet test CompactCryptoGroupAlgebra.Tests --logger "trx;LogFileName=TestResults.trx" --logger "nunit;LogFileName=TestResults.xml" --results-directory ./CompactCryptoGroupAlgebra.Tests/Coverage /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./Coverage/ /p:Exclude="[nunit.*]*"
rm -rf CompactCryptoGroupAlgebra.OpenSsl.Tests/Coverage
dotnet test CompactCryptoGroupAlgebra.OpenSsl.Tests --logger "trx;LogFileName=TestResults.trx" --logger "nunit;LogFileName=TestResults.xml" --results-directory ./CompactCryptoGroupAlgebra.OpenSsl.Tests/Coverage /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./Coverage/ /p:Exclude=\"[nunit.*]*,[CompactCryptoGroupAlgebra]*,[CompactCryptoGroupAlgebra.Tests]*\" /p:Include="CompactCryptoGroupAlgebra.OpenSsl*"
rm -rf CoverageReport
dotnet testPackages/ReportGenerator.4.6.1/tools/netcoreapp3.0/ReportGenerator.dll -reports:"CompactCryptoGroupAlgebra.Tests/Coverage/coverage.netcoreapp3.1.opencover.xml;CompactCryptoGroupAlgebra.OpenSsl.Tests/Coverage/coverage.netcoreapp3.1.opencover.xml" -targetdir:CoverageReport -reporttypes:HTML