.PHONY: all coverage
all: coverage

# === Config ========================================================= #

NUGET:=packages/nuget.exe
NUNIT:=packages/NUnit.ConsoleRunner.3.5.0/tools/nunit3-console.exe

# === Packages ======================================================= #

$(NUGET):
	mkdir -p packages
	curl https://api.nuget.org/downloads/nuget.exe -o $@

$(NUNIT): $(NUGET)
	cd packages;                          \
	mono nuget.exe install NUnit;         \
	mono nuget.exe install NUnit.Runners
	touch $@

# === Test =========================================================== #
.PHONY: test show

ASSEMBLIES:=BoundedLayers/bin/Debug/BoundedLayers.dll BoundedLayers.Test/bin/Debug/BoundedLayers.Test.dll
$(ASSEMBLIES): $(shell find -name '*.cs' | grep -v '/obj/')
	xbuild BoundedLayers.sln

test-results/coverage.covcfg: BoundedLayers.Test/coverage.covcfg; mkdir -p test-results; cp $< $@
test-results/coverage.covcfg.covdb: test-results/coverage.covcfg $(ASSEMBLIES) $(NUNIT)
	@BABOON_CFG=$(shell pwd)/test-results/coverage.covcfg mono  ../mono-af/XR.Baboon-master/covtool/bin/covem.exe $(NUNIT) BoundedLayers.Test/bin/Debug/BoundedLayers.Test.dll --labels=All
	@mv TestResult.xml test-results

test-results/html/index.html: test-results/coverage.covcfg.covdb
	(cd test-results; mono  ../../mono-af/XR.Baboon-master/covtool/bin/cov-html.exe ../$< BoundedLayers)

coverage: test-results/html/index.html

test: $(ASSEMBLIES) $(NUNIT)
	@echo "\n===> Running tests\n"
	@mkdir -p test-results/coverage
	mono $(NUNIT) BoundedLayers.Test/bin/Debug/BoundedLayers.Test.dll --labels=All
	@mv TestResult.xml test-results

show:
	xdg-open test-results/html/index.html

# === Version ======================================================== #
.PHONY: bump bump-asm
VERSION_FILES:=$(shell find -name AssemblyInfo.cs) \
	$(shell find -name '*.nuspec')             \
	$(shell find -name '*.csproj')
UPD_FILE:=                                                             \
	diff $$file.new $$file > /dev/null;                            \
	if [ $$? -ne 0 ]; then                                         \
		echo "  - $$file updated";                             \
		mv $$file.new $$file;                                  \
	else                                                           \
		rm $$file.new;                                         \
		touch $$file;                                          \
	fi
VERSION_SHELL:=cat BoundedLayers.sln | sed -ne 's/.*version = \([0-9.]*\).*/\1/p'

$(VERSION_FILES): BoundedLayers.sln
	@VER=`$(VERSION_SHELL)`;                                   \
	file=$@;                                                   \
	if `echo $$file | grep 'AssemblyInfo.cs' > /dev/null`; then \
		sed -e "s/^\[assembly: AssemblyVersion.*/[assembly: AssemblyVersion(\"$$VER.0\")]/" \
		    -e "s/^\[assembly: AssemblyFileVersion.*/[assembly: AssemblyFileVersion(\"$$VER.0\")]/" \
		    $$file > $$file.new;                               \
		$(UPD_FILE);                                           \
	fi;                                                        \
	if `echo $$file | grep '\.nuspec$$' > /dev/null`; then     \
		sed -e "s/<version>.*<\/version>/<version>$$VER<\/version>/" \
		    -e "s/\(<dependency id=\"BoundedLayers\".* version=\"\)[^\"]*\(\".*\)/\1$$VER\2/" \
		    $$file > $$file.new;                               \
		$(UPD_FILE);                                           \
	fi;                                                        \
	if `echo $$file | grep '\.csproj$$' > /dev/null`; then     \
		sed -e "s/<ReleaseVersion>.*<\/ReleaseVersion>/<ReleaseVersion>$$VER<\/ReleaseVersion>/" $$file > $$file.new; \
		$(UPD_FILE);                                           \
	fi

bump:
	@VER=`$(VERSION_SHELL)`;                                       \
	echo "Current version: $$VER";                                 \
	echo "New version: ${VERSION}";                                \
	file=BoundedLayers.sln;                                        \
	sed -e "s/version = \([0-9.]*\)/version = ${VERSION}/" $$file > $$file.new; \
	$(UPD_FILE)

bump-asm: $(VERSION_FILES)

# === Release ======================================================== #
.PHONY: release

SOURCES:=$(shell find -type f -path './BoundedLayers*' -not -path '*/bin/*' -not -path '*/obj/*' -not -name '*.nuspec')
RELEASE_FILES:=BoundedLayers/bin/Release/BoundedLayers.dll
release: $(RELEASE_FILES)


$(RELEASE_FILES): $(SOURCES)
	@echo "\n===> Building Release binaries for $@\n"
	xbuild /p:Configuration=Release BoundedLayers.sln

# === Package ======================================================== #
.PHONY: package push-package
PKG_SPECS:=$(shell find -name 'package.nuspec')
PKG_VER=$(shell $(VERSION_SHELL))
PKG_BIN=$(patsubst ./%/package.nuspec,pkg/%.$(PKG_VER).nupkg,$(PKG_SPECS))

package: $(PKG_BIN)

$(PKG_BIN) : pkg/%.$(PKG_VER).nupkg : %/package.nuspec $(RELEASE_FILES)
	@echo "\n===> Packaging binaries: $@ from $<\n"
	@mkdir -p pkg
	@cd pkg; mono ../$(NUGET) pack ../$< -Verbosity detailed

push-package: $(PKG_BIN)
	@for file in $^; do                                        \
		mono $(NUGET) push $$file -Verbosity detailed; \
	done

# === Clean ========================================================== #
.PHONY: clean
CLEAN_BINARIES:=$(shell find \( -name 'bin' -o -name 'obj' \) -a -type d)
clean:
	@if [ ! -z "$(CLEAN_BINARIES)" ]; then \
		echo Will remove: ;\
		echo -n '  - ' ;\
		echo $(CLEAN_BINARIES) | sed -e 's/ /\n  - /g' ; \
		rm -rI $(CLEAN_BINARIES); \
	fi
	rm -f pkg/*
	rm -rf test-results
