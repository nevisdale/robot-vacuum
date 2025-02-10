.PHONY: .bindeps
.bindeps:
	# https://github.com/Scony/godot-gdscript-toolkit
	pip3 install "gdtoolkit==4.*"

.PHONY: lint
lint:
	gdformat .
	# for windows
	find  | xargs dos2unix -s
