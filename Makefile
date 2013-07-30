all:
	mkdir -p bin/
	gmcs src/*.cs -out:bin/Net.dll -target:library

test:
	mkdir -p bin/
	gmcs src/*.cs test/*.cs -out:bin/Test.exe

clean:
	rm -fr bin/

.PHONY: all test clean
