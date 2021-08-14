cd ..

dotnet pack -o nupkg
dotnet tool install litedoc -g --add-source nupkg