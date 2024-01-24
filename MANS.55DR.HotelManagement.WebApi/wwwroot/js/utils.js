function formToJson(form) {
    return [...new FormData(form).entries()]
        .reduce((prev, current) => {
            prev[current[0]] = current[1];
            return prev;
        }, {});
}
//let test = [
//    ['name', 'TESTOWY'],
//    ['EMAIL','patryk@wp.pl']
//]

//let columns = [{
//    displayName: "Price",
//    sourceProperty: "price",
//    getColumnData: (rowData) => { return rowData.price },
//    getColumnHtmlData: (rowData) => `<button>Test</button>`
//},
//{
//    displayName: "Start Date",
//    sourceProperty: "startDate"
//}]

//let data = [
//    {
//        "customer": { name: "janek" },
//        id: 1,
//        price: 20,
//        startDate: new Date().toLocaleString()
//    }
//]

function createTable(elementId, columns, data, rowKeyPropName = "id") {
    const table = document.createElement("table");
    //create header
    const thead = table.createTHead();
    const headRow = document.createElement("tr");
    thead.appendChild(headRow);
    for (let column of columns) {
        const th = document.createElement("th");
        th.innerText = column.displayName;
        headRow.appendChild(th)
    }

    //creatae body
    const tbody = table.createTBody();
    for (let rowData of data) {
        const bodyRow = document.createElement("tr");
        for (let column of columns) {
            const td = document.createElement("td");
            if (column.getColumnHtmlData) {
                td.innerHTML = column.getColumnHtmlData(rowData);
            } else {
                td.innerText = column.getColumnData ? column.getColumnData(rowData) : rowData[column.sourceProperty];
            }
            bodyRow.appendChild(td)
        }
        tbody.appendChild(bodyRow);
    }

    const element = document.getElementById(elementId);
    element.appendChild(table);
};

function fillSelect(elementId, data, config) {
    const selectElem = document.getElementById(elementId);

    for (let optionData of data) {
        const option = document.createElement("option");
        option.value = optionData[config.valueProperty];
        option.innerText = config.getDisplayValue(optionData);
        selectElem.appendChild(option);
    }
}