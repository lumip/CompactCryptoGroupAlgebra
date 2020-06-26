#!/bin/bash
nuget restore CompactCryptoGroupAlgebra.sln
nuget install AltCover -Version 7.0.773 -OutputDirectory testPackages
nuget install NUnit.Console -Version 3.10.0 -OutputDirectory testPackages
nuget install ReportGenerator -Version 4.6.1 -OutputDirectory testPackages
msbuild /p:Configuration=Debug CompactCryptoGroupAlgebra.sln
mono testPackages/altcover.7.0.773/tools/net45/AltCover.exe -i CompactCryptoGroupAlgebra.Tests/bin/Debug/net46/ --localSource -e CompactCryptoGroupAlgebra.Tests
mono testPackages/NUnit.ConsoleRunner.3.10.0/tools/nunit3-console.exe CompactCryptoGroupAlgebra.Tests/bin/Debug/net46/__Instrumented/CompactCryptoGroupAlgebra.Tests.dll
mono testPackages/ReportGenerator.4.6.1/tools/net47/ReportGenerator.exe -reports:coverage.xml -targetdir:CoverageReport  
