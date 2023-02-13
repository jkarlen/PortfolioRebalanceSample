import React, { useState, useEffect } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faArrowUp, faArrowDown} from "@fortawesome/free-solid-svg-icons";

const ChangeColumn = (props) => {
    let icon;
    if (props.changePercentage > 0) {
        icon = <FontAwesomeIcon icon={faArrowUp}></FontAwesomeIcon>
    } else {
        icon = <FontAwesomeIcon icon={faArrowDown}></FontAwesomeIcon>
    }
    return (
        <td>{icon} {props.changePercentage}</td>
    )
}

export default ChangeColumn;