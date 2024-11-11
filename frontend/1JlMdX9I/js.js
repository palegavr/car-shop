

<script>
            // Отримуємо елементи чекбокса і вибору кольору
            const colorCheckbox = document.getElementById('topliv4');
            const colorInput = document.getElementById('exampleColorInput');
        
            // Відстежуємо зміну стану чекбокса
            colorCheckbox.addEventListener('change', () => {
                if (colorCheckbox.checked) {
                    // Додаємо атрибут `name`, коли чекбокс увімкнено
                    colorInput.setAttribute('name', 'different_car_color');
                } else {
                    // Видаляємо атрибут `name`, коли чекбокс вимкнено
                    colorInput.removeAttribute('name');
                }
            });

            const checkbox = document.getElementById("topliv4");

            checkbox.addEventListener('change', function() {
                if (checkbox.checked) {
                    // Если чекбокс включен, блокируем возможность его изменения на 300 миллисекунд
                    checkbox.disabled = true;

                    // Через 300 миллисекунд снимаем блокировку
                    setTimeout(function() {
                        checkbox.disabled = false;
                    }, 400);
                }
                else {
                    checkbox.disabled = true;

                    // Через 300 миллисекунд снимаем блокировку
                    setTimeout(function() {
                        checkbox.disabled = false;
                    }, 400);
                }
            });
            
            function updatePrice() {
                let totalPrice = 0;
                document.querySelectorAll('.option-checkbox').forEach(function(checkbox) {
                    if (checkbox.checked) {
                        totalPrice += parseFloat(checkbox.getAttribute('data-price')) || 0;
                    }
                });
                document.getElementById('pricefull').innerText = 'Цена: ' + totalPrice + ' грн';
            }

            document.querySelectorAll('.option-checkbox').forEach(function(checkbox) {
                checkbox.addEventListener('change', updatePrice);
            });

            // Initial calculation for pre-checked options
            updatePrice();
        </script>