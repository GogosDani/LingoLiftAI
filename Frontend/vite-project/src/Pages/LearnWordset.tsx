import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";

export default function LearnWordset() {
    const [wordset, setWordset] = useState();
    const { wordsetId } = useParams();

    return (
        <>
        </>
    );
}