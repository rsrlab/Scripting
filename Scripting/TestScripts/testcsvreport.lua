rowCount = 500
colCount = 10

Base.ResetCSV(colCount)
for row = 0, rowCount - 1 do
	Base.WriteCSV(row, 0, row)
	for col = 1 , colCount - 1 do
		Base.WriteCSV(row, col, col)
	end
end