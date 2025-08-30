#!/bin/sh

# Installs system-dependent development prerequisites.

set -eu

if [ -n "${GNU_TK_MSYS2_REPOSITORY_PREFIX-}" ]; then
    pacman -S --needed --noconfirm "${GNU_TK_MSYS2_REPOSITORY_PREFIX}fd"
    pacman -S --needed --noconfirm moreutils
fi
