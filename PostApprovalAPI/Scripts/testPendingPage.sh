page_size=100
for page in {1..100}; do
	curl "http://localhost:5000/admin/pending?page=$page&pageSize=$page_size"
done
