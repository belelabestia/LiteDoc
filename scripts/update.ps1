cd ..

dotnet pack -o nupkg
dotnet tool update litedoc -g --add-source nupkg 