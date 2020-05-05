
# CleanProject
A clone of the project from [kiwipiet's repository](https://github.com/kiwipiet/CleanProject)

Original source location [CleanProject - Cleans Visual Studio Solutions For Uploading or Email](https://code.msdn.microsoft.com/Clean-Cleans-Visual-Studio-a05bca4f)

Developed by Ron Jacobs. Last Version 1.2.3, Released on 2011.07.24, Works with Visual Studio 2010.

The project is refactored. The command line tool is removed. A simple WPF UI is created.

#Introduction
Clean Project is a utility that cleans Visual Studio project directories so you can quickly upload or email a zip file with your solution.

How many times have you wanted to send a project to a friend or upload it to a web site like MSDN Code Gallery only to find that your zip file has lots of stuff that you don't need to send in it making the file larger than it needs to be.

  * bin folder
  * obj folder
  * TestResults folder
  * Resharper folders

And then if you forget about removing Source Control bindings whoever gets your project will be prompted about that.  As someone who does this process a great deal I decided to share with you my code for cleaning a project.