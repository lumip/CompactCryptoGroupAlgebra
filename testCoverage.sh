#!/bin/bash
# SPDX-FileCopyrightText: Lukas Prediger
# SPDX-License-Identifier: CC0-1.0
set -e
dotnet restore
nuget install ReportGenerator -Version 4.6.1 -OutputDirectory testPackages
rm -rf CompactCryptoGroupAlgebra.Tests/Coverage
dotnet test CompactCryptoGroupAlgebra.Tests --logger "trx;LogFileName=TestResults.trx" --logger "nunit;LogFileName=TestResults.xml" --results-directory ./CompactCryptoGroupAlgebra.Tests/Coverage /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./Coverage/ /p:Exclude="[nunit.*]*"
rm -rf CompactCryptoGroupAlgebra.LibCrypto.Tests/Coverage
dotnet test CompactCryptoGroupAlgebra.LibCrypto.Tests --logger "trx;LogFileName=TestResults.trx" --logger "nunit;LogFileName=TestResults.xml" --results-directory ./CompactCryptoGroupAlgebra.LibCrypto.Tests/Coverage /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./Coverage/ /p:Exclude=\"[nunit.*]*,[CompactCryptoGroupAlgebra]*,[CompactCryptoGroupAlgebra.Tests]*\" /p:Include="CompactCryptoGroupAlgebra.LibCrypto*"
rm -rf CoverageReport
dotnet testPackages/ReportGenerator.4.6.1/tools/netcoreapp3.0/ReportGenerator.dll -reports:"CompactCryptoGroupAlgebra.Tests/Coverage/coverage.netcoreapp3.1.opencover.xml;CompactCryptoGroupAlgebra.LibCrypto.Tests/Coverage/coverage.netcoreapp3.1.opencover.xml" -targetdir:CoverageReport -reporttypes:HTML