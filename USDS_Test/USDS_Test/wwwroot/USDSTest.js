const agenciesURL = 'https://www.ecfr.gov/api/admin/v1/agencies.json';
const forwardURL_local = 'http://localhost:7194/api/ForwardRequest';
const forwardURL = 'https://usdstest.azurewebsites.net/api/ForwardRequest?code=9umIRvMVr-aw8J3dz52azO8waeCY3nxM3jfihpUgHFXjAzFu3lFBpQ=='
const agencyAnalysisURL_local = 'http://localhost:7194/api/AgencyAnalysis';
const agencyAnalysisURL = 'https://usdstest.azurewebsites.net/api/AgencyAnalysis?code=hd8mRG-IEoa8KHoaZQz0gkcQ3f9wshcgHzigevEc14PQAzFuvGM_3g==';

function btnAgencyAnalysis_OnClick() {

    let retVal = '';

    retVal = runAgencyAnalysis(divAgencies);

}

function btnGetAgencies_OnClick() {

    let retVal = '';

    retVal = getAgencies(divAgencies);

}

function JSONToHTMLTable(jsonData, table) {
    if (!table) {
        table = document.createElement('table');
        table.setAttribute('border', '1');
    }

    if (Array.isArray(jsonData)) {
        if (jsonData.length === 0) return table;
        const headers = Object.keys(jsonData[0]);
        let headerRow = document.createElement('tr');
        headers.forEach(headerText => {
            let header = document.createElement('th');
            header.textContent = headerText;
            headerRow.appendChild(header);
        });
        table.appendChild(headerRow);

        jsonData.forEach(item => {
            let dataRow = document.createElement('tr');
            headers.forEach(headerText => {
                let dataCell = document.createElement('td');
                let cellData = item[headerText];

                if (typeof cellData === 'object' && cellData !== null) {
                    if (Array.isArray(cellData) && cellData.length === 0) {
                        dataCell.textContent = '[]';
                    } else {
                        const nestedTable = JSONToHTMLTable(cellData);
                        dataCell.appendChild(nestedTable);
                    }
                } else {
                    dataCell.textContent = cellData;
                }
                dataRow.appendChild(dataCell);
            });
            table.appendChild(dataRow);
        });
    } else if (typeof jsonData === 'object' && jsonData !== null) {
        for (const key in jsonData) {
            if (jsonData.hasOwnProperty(key)) {
                let row = document.createElement('tr');
                let keyCell = document.createElement('th');
                keyCell.textContent = key;
                row.appendChild(keyCell);

                let valueCell = document.createElement('td');
                if (typeof jsonData[key] === 'object' && jsonData[key] !== null) {
                    const nestedTable = JSONToHTMLTable(jsonData[key]);
                    valueCell.appendChild(nestedTable);
                } else {
                    valueCell.textContent = jsonData[key];
                }
                row.appendChild(valueCell);
                table.appendChild(row);
            }
        }
    }
    return table;
}


function getWordCount(xmlNode) {
    if (!xmlNode) {
        return 0;
    }

    const textContent = xmlNode.textContent || xmlNode.innerText || "";
    const words = textContent.trim().split(/\s+/);

    return words.length;
}

function runAgencyAnalysis(outputElement) {

    let retVal = '';
    let outputTable = document.getElementById("outputTable");
    let outputDiv = document.getElementById("divOutputTable");
    const data = new FormData();
    data.append('hidUrl', agencyAnalysisURL);
    data.append('hidTest', 'Test');

    fetch(agencyAnalysisURL, {

        method: 'POST',
        body: data

    })
        .then(response => {
            if (response.status == 0) {
                //return response.json();
            }

            else if (!response.ok) {

                if (response.status === 404) {
                    throw new Error('Data not found');
                }

                else if (response.status === 500) {
                    throw new Error('Server error');
                }

                else {
                    throw new Error('Network response was not ok');
                }
            }

            return response.json();
        })
        .then(data => {
            outputElement.textContent = JSON.stringify(data, null, 2);

            JSONToHTMLTable(data, outputTable);

        })
        .catch(error => {
            console.error('Error:', error);
        });


    return retVal;

}

function getAgencies(outputElement) {

    let retVal = '';
    const data = new FormData();
    data.append('hidUrl', agenciesURL);
    data.append('hidTest', 'Test');

    retVal = forwardRequest(agenciesURL, outputElement);

    return retVal;
    
}


function forwardRequest(url, outputElement) {

    let retVal = '';
    let outputTable = document.getElementById("outputTable");
    let outputDiv = document.getElementById("divOutputTable");
    const data = new FormData();
    data.append('hidUrl', url);
    data.append('hidTest', 'Test');

    fetch(forwardURL, {

        method: 'POST',
        body: data

    })
        .then(response => {
            if (response.status == 0) {
                //return response.json();
            }

            else if (!response.ok) {

                if (response.status === 404) {
                    throw new Error('Data not found');
                }

                else if (response.status === 500) {
                    throw new Error('Server error');
                }

                else {
                    throw new Error('Network response was not ok');
                }
            }

            return response.json();
        })
        .then(data => {
            outputElement.textContent = JSON.stringify(data, null, 2);
            
            JSONToHTMLTable(data, outputTable);

        })
        .catch(error => {
            console.error('Error:', error);
        });

}

;
