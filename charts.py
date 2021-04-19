import matplotlib.pyplot as plt

x = ["8", "9", "10", "11"]
count = [1, 0, 3, 6]

x_pos = [i for i, _ in enumerate(x)]

plt.bar(x_pos, count, color = 'purple')
plt.xlabel("Android Version")
plt.ylabel("Number of Participants")
plt.title("Number of participants that use each android version")

plt.xticks(x_pos, x)

plt.savefig("D:/Documents/Uni-Notes/Dissertation/images/android-version-chart.png")