echo 'Running LiteDoc install script.'

dotnet pack -o nupkg
dotnet tool install litedoc -g --add-source nupkg

echo 'LiteDoc installed successfully.'
echo 'Usage: litedoc (run | watch | new) <working_path>'