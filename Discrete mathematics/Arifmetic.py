#Hardcode solution for arifmetic code
messenge = "abcbbbbbacabbacddacdbbaccbbadadaddd abcccccbacabbacbbaddbdaccbbddadadcc bcabbcdabacbbacbbddcbbaccbbdbdadaac@"
left = 0
right = 1
length = 1
prob = {0.23, 0.31, 0.23, 0.19, 0.02, 0.02}
for i in range(len(messenge)):
    if messenge[i] == 'a':
        right = left + length * 0.23
    elif messenge[i] == 'b':
        left += length * 0.23
        right = left + length * 0.31
    elif messenge[i] == 'c':
        left += length * 0.54
        right = left + length * 0.23
    elif messenge[i] == 'd':
        left += length * 0.77
        right = left + length * 0.19
    elif messenge[i] == ' ':
        left += length * 0.96
        right = left + length * 0.02
    else:
        left += length * 0.98
    length = right - left
print(left)        
