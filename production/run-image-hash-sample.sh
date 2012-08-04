#!/bin/sh

sample=image-hash-sample

output=$sample/output.txt

# clear the output file
cat /dev/null > $output

# store a list of hashes in the output file
for image in $sample/images/*
do
	hash=$(bin/image-hash $image)
	filename=$(basename $image)
	id=${filename%.*}
	echo $id\|$hash >> $output
done
