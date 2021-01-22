#!/bin/bash
# a helper utility for creating a release

# override commands which have pesky $path issues
find() {
	"/d/Software/dev/cmder/vendor/git-for-windows/usr/bin/find.EXE" "$@"
}
7z() {
	"/c/Program Files/7-Zip/7z.exe" "$@"
}

_toLower() {
	echo "$1" | tr '[:upper:]' '[:lower:]'
}

_getrid() {
	if [[ "$OSTYPE" == "msys" ]]; then
		echo "win-x64"
	elif [[ "$OSTYPE" == "win32" ]]; then
		echo "win-x64"
	else
		echo ""
	fi
}

_publishone() {
	if [ -z "$1" ]; then echo "_publishone invalid rid"; exit 1; fi
	if [ -z "$2" ]; then echo "_publishone invalid version"; exit 1; fi

	list="make.sh.tmp.txt"

	# do a build / publish with RID
	outNative="bin/Native/$1"
	dotnet publish -r "$1" -c release -o "$outNative"

	# package native build
	find "./$outNative/" -maxdepth 1 -type f > "$list"
	7z a -mx=9 -ms=on -i@"$list" "./$1-native-$2.7z"

	rm "$list"
}

publish() {
	# https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
	rid="$(_getrid)"
	#change version number here
	ver="1.0.1"

	if [ -n "$rid" ]; then
		_publishone "$(_getrid)" "$ver" "netcoreapp3.1"
	else
		echo "RID '$OSTYPE' not supported"
	fi
}

publish
