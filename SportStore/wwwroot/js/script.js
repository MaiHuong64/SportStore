 <script>
        // Navigation
        document.querySelectorAll('.sidebar .nav-link').forEach(link => {
            link.addEventListener('click', function(e) {
                if (this.getAttribute('data-page')) {
                    e.preventDefault();
                    
                    // Update active state
                    document.querySelectorAll('.sidebar .nav-link').forEach(l => l.classList.remove('active'));
                    this.classList.add('active');
                    
                    // Show selected page
                    const page = this.getAttribute('data-page');
                    document.querySelectorAll('.page-content').forEach(p => p.style.display = 'none');
                    const selectedPage = document.getElementById(page + '-page');
                    if (selectedPage) {
                        selectedPage.style.display = 'block';
                    }
                }
            });
        });
</script>