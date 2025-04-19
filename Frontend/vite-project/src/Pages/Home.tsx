import { useEffect } from "react"
import { api } from "../axios/api";
import { useNavigate } from "react-router-dom";

export default function Home() {

    const navigate = useNavigate();

    useEffect(() => {
        async function getUserLanguage() {
            const response = await api.get("/api/test/check-test-status");
            if (!response.data.testAvailable) navigate("/placement-test");
        }
        getUserLanguage();
    }, [])

    return (
        <>
            <p> HOME </p>
        </>
    )
}