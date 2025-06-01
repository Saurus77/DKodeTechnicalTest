document.addEventListener("DOMContentLoaded", function () {

    // Calculator state
    let currentInput = '0';
    let previousInput = '';
    let operation = null;
    let resetInput = false;
    let convertedValue = '';

    // Initialize currency rates (fetched from api)
    let rates = {
        GBP: 0,
        EUR: 0,
        USD: 0
    };

    const resultElement = document.getElementById('result');
    const historyElement = document.getElementById('previous-input');
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
    }

    // Initialize calculator
    function initCalculator() {
        fetchCurrencyRates();
        updateDisplay();

        // Event listeners for buttons
        buttons.forEach(button => {
            button.addEventListener('click', () => {
                const value = button.value;

                if (!isNaN(value) || value === ".") {   // If value is a number or period, append values
                    appendNumber(value);                
                } else if (value === 'C') {     // If value is a clear button, clear
                    clear();
                } else if (value === '=') {     // If value is equation button, calculate result  
                    calculate();
                } else if (['GBP', 'EUR', 'USD'].includes(value)) {     // If value is currency convert button, convert values
                    convertCurrency(value);
                } else {    // Else choose operation base on +,/,-,*
                    chooseOperation(value);
                }
                
                updateDisplay();
                updateHistoryDisplay(value);
            });
        });

    }


    // Calculator functions

    // Appends clicked values
    function appendNumber(number) {
        if (currentInput === '0' || resetInput) {     // If input is 0 or reset, then input is a value or if is period then put zero in front, else put number
            currentInput = number === '.' ? '0.' : number;
            resetInput = false;
        } else {
            if (number === '.' && currentInput.includes('.')) return;   // If value is period and current input includes period, do nothing
            currentInput += number;     // Put current input and given value together
        }
    }

    // Clear inputs/display
    function clear() {
        currentInput = '0';
        previousInput = '';
        operation = null;
        convertedValue = null;
    }

    // Sets what happens after user chooses operation
    function chooseOperation(op) {
        if (currentInput === '0') return;   // If current input is 0, do nothing
        if (previousInput !== '') {     // If previous input is not empty, then calculate
            calculate();
        }

        operation = op;
        previousInput = currentInput;
        resetInput = true;
    }

    function calculate() {
        let calculation;
        const previous = parseFloat(previousInput);     // Parse previous input to float
        const current = parseFloat(currentInput);   // Parse current input to float

        if (isNaN(previous) || isNaN(current)) return;  // If previous input is not a number or current is not a number, then do nothing

        // Basic math operations
        switch (operation) {
            case '+': calculation = previous + current; break;

            case '-': calculation = previous - current; break;

            case '*': calculation = previous * current; break;

            case '/': calculation = previous / current; break;

            default: return;
        }

        currentInput = parseFloat(calculation.toFixed(2)).toString();   // Current input parset to float for accuracy and limited to decimal digits
        operation = null;
        previousInput = '';
        resetInput = true;
    }

    // Converts currency
    function convertCurrency(currency) {
        const amount = parseFloat(currentInput);    // Parses current input to float

        if (isNaN(amount)) return;  // If value is not a number, do nothing

        if (rates[currency] > 0) {     // If rate for given currency is greater than zero, then convert
            const converted = amount * rates[currency]; // Conversion
            convertedValue = { amount: amount, currency: currency, value: converted.toFixed(2) };   // Assign info of conversed values: amount, currency and limit decimal for value to 2
            resetInput = true;
        } else {
            alert('Currency rates not loaded, please try again.');
        }
 
    }

    // Updates display with current input
    function updateDisplay() {
        resultElement.value = currentInput;
    }

    // Updates history display
    function updateHistoryDisplay(value) {
        // Currency conversion info
        if (convertedValue) {
            historyElement.value = `${convertedValue.amount} PLN = ${convertedValue.value} ${convertedValue.currency}`; // Display converted value from PLN to chosen currency in history element
            convertedValue = '';
            return;
        } else {
            historyElement.value = '';
        }
    }
   
    initCalculator();

});