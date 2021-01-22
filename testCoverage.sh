#!/bin/bash
nuget restore CompactCryptoGroupAlgebra.sln
nuget install AltCover -Version 7.0.773 -OutputDirectory testPackages
nuget install NUnit.Console -Version 3.10.0 -OutputDirectory testPackages
nuget install ReportGenerator -Version 4.6.1 -OutputDirectory testPackages
msbuild /p:Configuration=Debug CompactCryptoGroupAlgebra.sln

rm coverage.xml
mono testPackages/altcover.7.0.773/tools/net45/AltCover.exe -i CompactCryptoGroupAlgebra.Tests/bin/Debug/net46/ --localSource -e CompactCryptoGroupAlgebra.Tests -x coverage.xml
mono testPackages/NUnit.ConsoleRunner.3.10.0/tools/nunit3-console.exe CompactCryptoGroupAlgebra.Tests/bin/Debug/net46/__Instrumented/CompactCryptoGroupAlgebra.Tests.dll

rm openssl-coverage.xml
mono testPackages/altcover.7.0.773/tools/net45/AltCover.exe -i CompactCryptoGroupAlgebra.OpenSsl.Tests/bin/Debug/net46/ --localSource -e CompactCryptoGroupAlgebra.OpenSsl.Tests -x "openssl-coverage.xml"
mono testPackages/NUnit.ConsoleRunner.3.10.0/tools/nunit3-console.exe CompactCryptoGroupAlgebra.OpenSsl.Tests/bin/Debug/net46/__Instrumented/CompactCryptoGroupAlgebra.OpenSsl.Tests.dll

mono testPackages/ReportGenerator.4.6.1/tools/net47/ReportGenerator.exe -reports:"coverage.xml;openssl-coverage.xml" -targetdir:CoverageReport  
