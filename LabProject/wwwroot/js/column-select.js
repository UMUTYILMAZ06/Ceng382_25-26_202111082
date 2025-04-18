document.addEventListener("DOMContentLoaded", function () {
    const selectedColumns = new Set();
    const selectedInput = document.getElementById("SelectedColumnsInput");

    document.querySelectorAll(".selectable-column").forEach(th => {
        th.addEventListener("click", function () {
            const column = th.getAttribute("data-column");
            if (selectedColumns.has(column)) {
                selectedColumns.delete(column);
                th.classList.remove("table-active");
            } else {
                selectedColumns.add(column);
                th.classList.add("table-active");
            }
            selectedInput.value = Array.from(selectedColumns).join(",");
        });
    });
});
