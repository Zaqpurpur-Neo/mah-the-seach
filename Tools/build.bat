@echo off

clang++ -o ./scanner ./scanner.cpp -std=gnu++17 -municode -mconsole -DUNICODE -D_UNICODE -lole32 -lshell32 -luuid -lgdi32 -lgdiplus
