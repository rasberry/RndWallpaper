#!/bin/bash
# a helper utility for creating a release

# override commands which have pesky $path issues
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
	if [[ -z "$1" ]]; then echo "_publishone invalid rid"; exit 1; fi
	if [[ -z "$2" ]]; then echo "_publishone invalid version"; exit 1; fi

	local zipFile="./$1-native-$2.7z"
	# if [[ -f "$zipFile" ]]; then echo "$zipFile already exists"; exit 2; fi

	list="make.sh.tmp.txt"

	# do a build / publish with RID
	exec 5>&1
	out=$(dotnet publish -r "$1" -c release -p:Version=$ver|tee /dev/fd/5)
	if [[ $? != 0 ]]; then return; fi

	while read -r line
	do
		if [[ "$line" == *"RndWallpaper"* ]] && [[ "$line" == *"publish"* ]]; then
			outNative=${line#*-> }
		fi
	done <<< "$out"

	if [[ -z "$outNative" ]]; then
		echo "Did not spot publish folder in output"
		return
	fi

	# package native build
	7z a -mx=9 -ms=on "$zipFile" "$outNative"
}

publish() {
	# https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
	rid="$(_getrid)"
	#change version number here
	ver="2.0.0"

	if [ -n "$rid" ]; then
		_publishone "$(_getrid)" "$ver" "net7.0"
	else
		echo "RID '$OSTYPE' not supported"
	fi
}

publish
