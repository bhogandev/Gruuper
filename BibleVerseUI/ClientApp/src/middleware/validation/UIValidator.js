//SUMMARY - Verify the field passed is not empty
export function fieldIsNotEmpty(field){
    return field === '' ? false : true;
}

//SUMMARY - Verify the field passed does not go over the provided character limit
export function fieldHasTooManyCharacters(field, charLimit){
    return field.length > charLimit ? true : false;
}

//SUMMARY - Verify the field passed does not contain any of the listed special characters 
export function fieldContainsSpecialCharacters(field){
    const specialChars = /[ `!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/;
    return specialChars.test(field);
}