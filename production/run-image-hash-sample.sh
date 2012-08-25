#!/bin/bash

sample=image-hash-sample

output=$sample/output.txt

# clear the output file
cat /dev/null > $output

# store a list of hashes in the output file
for image in $sample/images/*
do
	rotate=$(exiftran -d $image  2>/dev/null | grep Orientation | grep -i --count 'bottom\|right')
	if [ "$rotate" != "0" ]; then
		/usr/bin/exiftran -a $image -o /var/tmp/auto-rotated.tmp
		hash=$(bin/image-hash /var/tmp/auto-rotated.tmp)
	else 
		hash=$(bin/image-hash $image)
	fi
	filename=$(basename $image)
	id=${filename%.*}
	echo $id\|$hash\|$rotate >> $output
done
