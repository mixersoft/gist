#!/bin/sh

cat sample/venice.json | bin/image-group --base_path sample/ --preserve_order --pretty_print > sample/output-ordered.json
