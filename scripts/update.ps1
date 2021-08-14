echo 'Running LiteDoc update script.'

dotnet pack -o nupkg
dotnet tool update litedoc -g --add-source nupkg

echo 'LiteDoc updated successfully.'