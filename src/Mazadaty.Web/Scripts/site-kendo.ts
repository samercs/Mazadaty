class KendoFormatter {

    static getYesNo(value: boolean) {
        if (value) {
            return "<span class='text-yes'>Yes</span>";
        } else {
            return "<span class='text-no'>No</span>";
        }
    }

    static formatDate(value: Date) {
        return "xxx";
    }
}