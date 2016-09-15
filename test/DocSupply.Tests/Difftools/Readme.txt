Test project uses shouldy for assertions : https://shouldly.readthedocs.io/en/latest/

Uses the approval functionality : https://shouldly.readthedocs.io/en/latest/assertions/shouldMatchApproved.html

The visual studio diff merge tool, vsDiffMerge.exe has been included in the project so that these tests can be run
by a build server

Shouldy recommends KDiff3, this is used in development. Allows developer to see and correct any assertions.
It is recommended that this is downloaded and used : http://kdiff3.sourceforge.net/

As long as this exists in C:\Program Files\KDiff3 Shouldy will pick this up and use it.