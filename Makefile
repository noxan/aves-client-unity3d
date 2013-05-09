all:
	mkdir -p bin/
	gmcs src/*.cs -out:bin/Net.dll -target:library

clean:
	rm -fr bin/

.PHONY: all clean
