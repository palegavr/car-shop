<script>
            async function confirmDeletion() {
                const errorMessage = document.getElementById('errorMessage');
                
                try {
                    const response = await fetch(`/api/admin/car/@car.Id`, { 
                        method: 'DELETE'
                    });
        
                    if (response.status === 200) {
                        window.location.href = '/catalog';
                    } else {
                        errorMessage.style.display = 'block';
                    }
                } catch (error) {
                    errorMessage.style.display = 'block';
                }
            }
        </script>