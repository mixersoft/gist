#!/bin/bash

cd api

# delete any vestiges of the old API

for dir in php-server js-client cs-client
do
	rm -f -r $dir
	mkdir $dir
done

# generate new API

src=Tasks.thrift

thrift --gen php:server -out php-server $src
thrift --gen csharp -out cs-client $src
thrift --gen js:jquery -out js-client $src
