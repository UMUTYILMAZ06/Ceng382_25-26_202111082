document.addEventListener("DOMContentLoaded", function () {
    const columnHeaders = document.querySelectorAll(".selectable-column");
    const selectedColumnsInput = document.getElementById("SelectedColumnsInput");

    const selectedColumns = new Set();

    function toggleColumnHighlight(columnName, isSelected) {
        const cells = document.querySelectorAll(`.col-${columnName}`);
        cells.forEach(cell => {
            if (isSelected) {
                cell.classList.add("bg-primary", "text-white");
            } else {
                cell.classList.remove("bg-primary", "text-white");
            }
        });
    }

    columnHeaders.forEach(header => {
        header.addEventListener("click", () => {
            const columnName = header.getAttribute("data-column");

            if (selectedColumns.has(columnName)) {
                selectedColumns.delete(columnName);
                header.classList.remove("bg-primary", "text-white");
                toggleColumnHighlight(columnName, false);
            } else {
                selectedColumns.add(columnName);
                header.classList.add("bg-primary", "text-white");
                toggleColumnHighlight(columnName, true);
            }

            selectedColumnsInput.value = Array.from(selectedColumns).join(",");
        });
    });
});
