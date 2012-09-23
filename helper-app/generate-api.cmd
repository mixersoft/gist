@echo off
setlocal

cd api

:: delete vestiges of the old API

for %%x in (php-server js-client cs-client) do (
	rmdir/s/q %%x
	mkdir %%x
) 2> nul

:: generate new API

set src=Tasks.thrift

thrift --gen php:server -out php-server %src%
thrift --gen csharp -out cs-client %src%
thrift --gen js:jquery -out js-client %src%
