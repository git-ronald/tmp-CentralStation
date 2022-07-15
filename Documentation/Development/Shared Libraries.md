# Shared Libraries

Important Notes
---------------
- Some solutions contain a SharedLibraries subfolder with projects that all have their own git repository.
  Before you can use such a soltution, you have to first clone those repositories into the SharedLibraries subfolder.

Instructions For Creating A New Shared Library
----------------------------------------------
- In Visual Studio, creaete a new class library. As usual, Visual Studio will create both a project and a solution file.
- Add the solution to a new git repository.
- Remove the solution file.
- Move everything under de project folder to the solution folder (except bin and obj).
- Remove the project folder.
- Commit and push the changes to git.
- Remove the entire solution folder (including the local git repository).
- Of course, to be able to develop on the project you have to first import it into another solution. Follow these steps:
  - Open the solution in Visual Studio.
  - Add a solution folder "SharedLibraries" if it doesn't already exist.
  - Clone the repository of the new shared library into that subfolder.
  - In Visual Studio, add the project to the solution.
