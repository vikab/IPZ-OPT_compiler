.386
DATA SEGMENT USE16
@2 label dword
@4 label dword
@1 label dword
DATA ENDS
CODE SEGMENT USE16
ASSUME CS:CODE, DS:DATA
BEGIN:
mov ax,data
mov ds,ax
TESTPOC proc
nop
ret
TESTPOC END
mov ax,4c00h
int 21h
CODE ENDS
END BEGIN
