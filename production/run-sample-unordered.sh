#!/bin/sh

sample=image-group-sample
cat $sample/venice.json | bin/image-group --base_path $sample/ --pretty_print > $sample/output-unordered.json
