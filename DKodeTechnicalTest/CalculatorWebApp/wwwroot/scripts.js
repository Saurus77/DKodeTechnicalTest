document.addEventListener("DOMContentLoaded", function () {

    // Calculator state
    let currentInput = 0;
    let previousInput = '';
    let operation = null;
    let resetInput = false;

    // Initialize currency rates (fetched from api)
    let rates = {
        GBP: 0,
        EUR: 0,
        USD: 0
    };

    const resultElement = document.getElementById('result');
    const buttons = document.querySelectorAll('#calculator input[type="button"]');

    // Fetch currency rates from NBP API

    // ! Currency rates from 2025-03-02 are unavailable in NBP API (404) -> calculator will take most recent rates 

    async function fetchCurrencyRates() {
        try {
            // Fetch from api
            const gbpResponse = await fetch('https://api.nbp.pl/api/exchangerates/rates/a/gbp/?format=json');
            const eurResponse = await fetch('https://api.nbp.pl/api/exchangerates/rates/a/eur/?format=json');
            const usdResponse = await fetch('https://api.nbp.pl/api/exchangerates/rates/a/usd/?format=json');

            // Retrieve json data
            const gbpData = await gbpResponse.json();
            const eurData = await eurResponse.json();
            const usdData = await usdResponse.json();

            // Assing json data to rate variables
            rates.GBP = gbpData.rates[0].mid;
            rates.EUR = eurData.rates[0].mid;
            rates.USD = usdData.rates[0].mid;
        }
        catch (error) {
            console.error('Error while fetching currency rates:', error);
            console.error('Applying default rates - may not be viable');
            rates = { GBP: 5.00, EUR: 4.00, USD: 3.00 }; // Default rates
        }

        // Initialize calculator
        function initCalculator() {
            fetchCurrencyRates();
            updateDisplay();

            // Event listeners for buttons
            buttons.forEach(button => {
                button.addEventListener('click', () => {
                    const value = button.value;

                    if (!isNaN(value) || value === ".") {
                        appendNumber(value);
                    } else if (value === 'C') {
                        clear();
                    } else if (value === '=') {
                        calculate();
                    } else if (['GBP', 'EUR', 'USD'].includes(value)) {
                        convertCurrency(value);
                    } else {
                        chooseOperation(value);
                    }

                    updateDisplay();
                });
            });
        }

        // Calculator functions
        function appendNumber(number) {
            if (currentInput === '0' || resetInput) {
                currentInput = number === '.' ? '0.' : number;
                resetInput = false;
            } else {
                if (number === '.' && currentInput.includes('.')) return;
                currentInput = + number;
            }
        }

        function clear() {
            currentInput = '0';
            previousInput = '';
            operation = null;
        }

        function chooseOperation(op) {
            if (currentInput === '0') return;
            if (previousInput !== '') {
                calculate();
            }

            operation = op;
            previousInput = currentInput;
            resetInput = true;
        }

        function calculate() {
            let calculation;
            const previous = parseFloat(previousInput);
            const current = parseFloat(currentInput);

            if (isNaN(previous) || isNaN(current)) reutrn;

            switch (operation) {
                case '+':
                    calculation = previous + current;
                    break;

                case '-':
                    calculation = previous - current;
                    break;

                case '*':
                    calculation = previous * current;
                    break;

                case '/':
                    calculation = previous / current;
                    break;

                default:
                    return;
            }

            currentInput = calculation.toString();
            operation = null;
            previousInput = '';
            resetInput = true;
        }

        function convertCurrency(currency) {
            const amount = parseFloat(currentInput);

            if (isNaN(amout)) reutnr;

            if (rates[currency] > 0) {
                const converted = amout * rates[currency];
                currentInput = converted.toFixed(4).toString();
                resetInput = true;

                showConversionMessage(amount, currency, converted);
            } else {
                alert('Currency rates not loaded. Please try again');
            }
        }

        function showConversionMessage(amount, currency, converted) {
            const currencyNames = {
                GBP: 'British Pound',
                EUR: 'Euro',
                USD: 'US Dollar'
            };

            const message = `${amount.toFixed(2)} PLN = ${converted} ${currencyNames[currency]}`;
            console.log(message);
        }

        function updateDisplay() {
            resultElement.value = currentInput;
        }

        initCalculator();
    }
}